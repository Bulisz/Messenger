import { Component, OnDestroy, OnInit } from '@angular/core';
import { MessageModel } from 'src/app/models/message-model';
import { SenderReceiverModel } from 'src/app/models/sender-receiver-model';
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
  userName = ''
  set ready(isReady: boolean) {
    console.log(this.ready)
    if (isReady) this.scrollToBottom();
  }

  constructor(private auth: AuthService, private ms: MessageService, private lss: LocalStorageService){ }

  async ngOnInit() {
    this.auth.user.subscribe({
      next: async un => {
        this.userName = un?.userName as string
        let senderreceiver: SenderReceiverModel = {
          senderUserName: this.userName,
          receiverUserName: this.lss.getReceiver()
        }
    
        await this.ms.getPrivateMessages(senderreceiver)
          .then(res => {
            this.messages = res
          })

        setTimeout(()=>this.scrollToBottom(),100)
      }
    })

    setTimeout(async ()=> await this.auth.hubConnection?.invoke('JoinPrivateMessage', this.lss.getReceiver()), 1000)
    setTimeout(async ()=> this.auth.hubConnection?.on("ReceiveMessageFromUser", (res: MessageModel) => {
      this.messages.push(res)
      setTimeout(()=>this.scrollToBottom(),25)
  }), 1000)

  }

  scrollToBottom() {
    const lastElemId = 'mes' + (this.messages.length - 1)
    var elem = document.getElementById(lastElemId) as HTMLElement
    elem.scrollIntoView()
  }

  async ngOnDestroy() {
    await this.auth.hubConnection?.invoke('LeavePrivateMessage', this.lss.getReceiver())
    this.auth.hubConnection?.off("ReceiveMessageFromUser")
  }
}
