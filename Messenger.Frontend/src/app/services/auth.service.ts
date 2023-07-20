import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { LoginRequestModel } from '../models/login-request-model';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { UserModel } from '../models/user-model';
import { TokensModel } from '../models/tokens-model';
import { CreateUserModel } from '../models/create-user-model';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { GoogleLoginModel } from '../models/google-login-model';
import { CreateGoogleUserModel } from '../models/create-google-user-model';
import { LocalStorageService } from './local-storage.service';
import { LogoutRefreshRequestModel } from '../models/logout-refresh-request-model';
import { MessageService } from './message.service';
import { MessageModel } from '../models/message-model';
import { UnreadedMessageModel } from '../models/unreaded-message-model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  BASE_URL = environment.apiUrl + 'users/'
  user = new BehaviorSubject<UserModel | null>(null)
  hubConnection: HubConnection | null = null
  connectedUsers = new BehaviorSubject<number>(0);
  unreadedMessages = new BehaviorSubject<Array<UnreadedMessageModel>>([]);

  constructor(private http: HttpClient, private lss: LocalStorageService, private ms: MessageService) { }

  async getUsers(): Promise<any> {
    return await firstValueFrom(this.http.get<Array<string>>(`${this.BASE_URL}getusers`))
  }

  async getCurrentUser(): Promise<any> {
    if (this.lss.getAccessToken()) {
      await firstValueFrom(this.http.get<UserModel>(`${this.BASE_URL}getcurrentuser`))
        .then(async um => {
          this.user.next(um)
          this.lss.setUser(um.userName)
          if (!this.hubConnection?.connectionId) {
            await this.startHubConnection()
              .then(() => {
                this.hubConnection?.on('ReceiveConnectedUsersNumber', res => this.connectedUsers.next(res))
                this.hubConnection?.on('GetUnreadedMessages',() => {
                  console.log('asdasd')
                  this.ms.getUnreadedMessages(this.user.value?.userName as string)
                  .then(res => this.unreadedMessages.next(res))
                })
              })
          }
          if (this.lss.getMode() === 'private') {
            this.hubConnection?.off("ReceiveMessageFromUser")
            await this.hubConnection?.invoke('JoinPrivateMessage', this.lss.getReceiver())
            this.hubConnection?.on("ReceiveMessageFromUser", (res: MessageModel) => {
              let actualMessages: Array<MessageModel> = this.ms.messages.value
              actualMessages.push(res)
              this.ms.messages.next(actualMessages)
            })
          }
          await this.ms.getUnreadedMessages(um.userName)
            .then(res => this.unreadedMessages.next(res))
        })
        .catch(async err => {
          if (err.status !== 401) {
            this.lss.clearAccessToken()
            this.lss.clearRefreshToken()
            this.user.next(null)
            if (this.hubConnection) {
              this.hubConnection?.off
              this.hubConnection?.stop()
            }
          }
        })
    }
  }

  async refresh(): Promise<any> {
    let refreshToken = this.lss.getRefreshToken()
    return await firstValueFrom(this.http.post<TokensModel>(`${this.BASE_URL}refresh`, { refreshToken }))
      .then(async td => {
        if (td.accessToken && td.refreshToken) {
          this.lss.setAccessToken(td.accessToken.value)
          this.lss.setRefreshToken(td.refreshToken.value)
          this.getCurrentUser()
        }
      })
      .catch(async () => {
        this.lss.clearAccessToken()
        this.lss.clearRefreshToken()
        if (this.hubConnection) {
          await this.hubConnection.stop()
        }
      })
  }

  async registerNewAccout(newAccount: CreateUserModel): Promise<any> {
    return await firstValueFrom(this.http.post<UserModel>(`${this.BASE_URL}register`, newAccount));
  }

  async googleRegister(userModel: CreateGoogleUserModel): Promise<any> {
    return await firstValueFrom(this.http.post<TokensModel>(`${this.BASE_URL}googleregister`, userModel))
      .then(async lrm => {
        if (lrm.accessToken) {
          this.lss.setAccessToken(lrm.accessToken.value)
          this.lss.setRefreshToken(lrm.refreshToken.value)
          await this.getCurrentUser()
        }
      })
  }

  async login(loginModel: LoginRequestModel): Promise<any> {
    return firstValueFrom(this.http.post<TokensModel>(`${this.BASE_URL}login`, loginModel))
      .then(async tm => {
        if (tm.accessToken) {
          this.lss.setAccessToken(tm.accessToken.value)
          this.lss.setRefreshToken(tm.refreshToken.value)
          await this.getCurrentUser()
        }
      })
  }

  async loginWithGoogle(credentials: GoogleLoginModel): Promise<any> {
    return await firstValueFrom(this.http.post<TokensModel>(`${this.BASE_URL}googlelogin`, credentials))
      .then(async lrm => {
        if (lrm.accessToken) {
          this.lss.setAccessToken(lrm.accessToken.value)
          this.lss.setRefreshToken(lrm.refreshToken.value)
          await this.getCurrentUser()
        }
      })
  }

  async startHubConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}?access_token=${this.lss.getAccessToken()}`)
      .build()
    await this.hubConnection.start()
  }

  async logout(): Promise<any> {
    let refreshModel: LogoutRefreshRequestModel = { refreshToken: this.lss.getRefreshToken() };
    await firstValueFrom(this.http.post(`${this.BASE_URL}logout`, { refreshModel }))
    this.lss.clearAccessToken()
    this.lss.clearRefreshToken()
    this.user.next(null)
    this.hubConnection?.off
    this.hubConnection?.stop()
  }
}
