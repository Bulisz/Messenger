import { Component, Input, OnInit } from '@angular/core';
import { MessageModel } from 'src/app/models/message-model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit {

  @Input() receiver = ''
  messages: Array<MessageModel> = []
  userName = ''

  constructor(private auth: AuthService){}

  async ngOnInit() {
    this.userName = this.auth.user.value?.userName as string
    await this.auth.hubConnection?.invoke('JoinPrivateMessage', this.receiver)
    this.auth.hubConnection?.on("ReceiveMessageFromUser", (res: MessageModel) => {
      if(res.senderUserName !== this.userName){
        this.messages.push(res)
      }
    })
  }

  async sendPrivateMessage(){
    let messageModel:MessageModel = {
      senderUserName: this.userName,
      receiverName: this.receiver,
      text: 'valami'
    }
    await this.auth.hubConnection?.invoke('SendPrivateMessage',messageModel)
  }
}
