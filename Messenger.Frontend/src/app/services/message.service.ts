import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from 'src/environments/environment';
import { MessageModel } from '../models/message-model';
import { SenderReceiverModel } from '../models/sender-receiver-model';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  BASE_URL = environment.apiUrl + 'messages/'

  constructor(private http: HttpClient){}

  async getPrivateMessages(senderreceiver: SenderReceiverModel): Promise<any> {
    return await firstValueFrom(this.http.post<Array<MessageModel>>(`${this.BASE_URL}GetPrivateMessages`,senderreceiver))
  }
}
