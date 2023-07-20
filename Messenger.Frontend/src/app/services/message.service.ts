import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { environment } from 'src/environments/environment';
import { MessageModel } from '../models/message-model';
import { SenderReceiverModel } from '../models/sender-receiver-model';
import { LocalStorageService } from './local-storage.service';
import { UnreadedMessageModel } from '../models/unreaded-message-model';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  BASE_URL = environment.apiUrl + 'messages/'

  constructor(private http: HttpClient, private lss: LocalStorageService){}
  messages = new BehaviorSubject<Array<MessageModel>>([])
  mode = new BehaviorSubject<string>('')

  async getPrivateMessages(sender: string): Promise<any> {
    let senderreceiver: SenderReceiverModel = {
      senderUserName: sender,
      receiverUserName: this.lss.getReceiver()
    }
    await firstValueFrom(this.http.post<Array<MessageModel>>(`${this.BASE_URL}GetPrivateMessages`,senderreceiver))
      .then(res => this.messages.next(res))
  }

  async getUnreadedMessages(userName: string): Promise<any> {
    let model : SenderReceiverModel = {
      senderUserName: userName,
      receiverUserName: userName
    }
    return await firstValueFrom(this.http.post<Array<UnreadedMessageModel>>(`${this.BASE_URL}GetUnreadedMessages`,model))
  }
}
