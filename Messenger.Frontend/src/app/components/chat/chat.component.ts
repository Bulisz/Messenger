import { Component, OnDestroy, OnInit } from '@angular/core';
import { MessageModel } from 'src/app/models/message-model';
import { AuthService } from 'src/app/services/auth.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, OnDestroy {

  messages: Array<MessageModel> = []

  constructor(public auth: AuthService, private ms: MessageService, private lss: LocalStorageService){ }

  async ngOnInit() {
    this.ms.messages.subscribe({
      next: res => {
        this.messages = res
        this.scrollToBottom()
      }
    })
    
    this.ms.getPrivateMessages(this.lss.getUser())
  }

  scrollToBottom() {
    const lastElemId = 'mes' + (this.messages.length - 1)
    setTimeout(() => {
      var elem = document.getElementById(lastElemId) as HTMLElement
      if (elem){
        elem.scrollIntoView()
      }
    },0)
  }

  async ngOnDestroy() {
    if(this.auth.hubConnection){
      await this.auth.hubConnection?.invoke('LeavePrivateMessage', this.lss.getReceiver())
    }
    this.auth.hubConnection?.off("ReceiveMessageFromUser")
    this.ms.mode.next('')
  }
}
