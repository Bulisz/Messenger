import { Component, Input, OnInit } from '@angular/core';
import { MessageModel } from 'src/app/models/message-model';
import { SenderReceiverModel } from 'src/app/models/sender-receiver-model';
import { AuthService } from 'src/app/services/auth.service';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit {

  messages: Array<MessageModel> = []
  userName = ''

  constructor(private auth: AuthService, private ms: MessageService){}

  async ngOnInit() {
    this.userName = this.auth.user.value?.userName as string
    await this.auth.hubConnection?.invoke('JoinPrivateMessage', this.ms.receiver)
    this.auth.hubConnection?.on("ReceiveMessageFromUser", (res: MessageModel) => {
        this.messages.push(res)
    })

    let senderreceiver: SenderReceiverModel = {
      senderUserName: this.userName,
      receiverUserName: this.ms.receiver
    }

    await this.ms.getPrivateMessages(senderreceiver)
      .then(res => this.messages = res)
  }
}
