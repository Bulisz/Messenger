import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { LoginRequestModel } from '../models/login-request-model';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { UserModel } from '../models/user-model';
import { TokensModel } from '../models/tokens-model';
import { CreateUserModel } from '../models/create-user-model';
import { HubConnection } from '@microsoft/signalr';
import { GoogleLoginModel } from '../models/google-login-model';
import { CreateGoogleUserModel } from '../models/create-google-user-model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  BASE_URL = environment.apiUrl + 'users/'
  user = new BehaviorSubject<UserModel | null>(null)

  constructor(private http: HttpClient, public hc: HubConnection) { }

  async getCurrentUser(): Promise<any> {
    return await firstValueFrom(this.http.get<UserModel>(`${this.BASE_URL}getcurrentuser`))
      .then(um => {
        this.user.next(um)
        if (localStorage.getItem('refreshToken')) {
          this.hc.start()
        }
      })
      .catch(() => this.logout())
  }

  async refresh(): Promise<any> {
    let refreshToken = localStorage.getItem('refreshToken') ? localStorage.getItem('refreshToken') : '';
    return await firstValueFrom(this.http.post<TokensModel>(`${this.BASE_URL}refresh`, { refreshToken }))
      .then(async td => {
        if (td.accessToken && td.refreshToken) {
          localStorage.setItem('accessToken', td.accessToken.value);
          localStorage.setItem('refreshToken', td.refreshToken.value);
          await this.getCurrentUser()
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
          localStorage.setItem('accessToken', lrm.accessToken.value);
          localStorage.setItem('refreshToken', lrm.refreshToken.value);
          await this.getCurrentUser()
        }
      })
  }

  async login(loginModel: LoginRequestModel): Promise<any> {
    return firstValueFrom(this.http.post<TokensModel>(`${this.BASE_URL}login`, loginModel))
      .then(async tm => {
        if (tm.accessToken) {
          localStorage.setItem('accessToken', tm.accessToken.value);
          localStorage.setItem('refreshToken', tm.refreshToken.value);
          await this.getCurrentUser()
        }
      })
  }

  async loginWithGoogle(credentials: GoogleLoginModel): Promise<any> {
    return await firstValueFrom(this.http.post<TokensModel>(`${this.BASE_URL}googlelogin`, credentials))
      .then(async lrm => {
        if (lrm.accessToken) {
          localStorage.setItem('accessToken', lrm.accessToken.value);
          localStorage.setItem('refreshToken', lrm.refreshToken.value);
          await this.getCurrentUser()
        }
      })
  }

  async logout(): Promise<any> {
    let refreshToken = localStorage.getItem('refreshToken') ? localStorage.getItem('refreshToken') : '';
    await firstValueFrom(this.http.post(`${this.BASE_URL}logout`, { refreshToken }))
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.user.next(null)
    this.hc.stop()
  }
}
