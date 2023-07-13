import { Component } from '@angular/core';
import { MessageModel } from 'src/app/models/message-model';
import { AuthService } from 'src/app/services/auth.service';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent {

  inputContent=''

  constructor(private auth: AuthService, private ms: MessageService){}

  async sendMessage(){
    let messageModel:MessageModel = {
      senderUserName: this.auth.user.value?.userName as string,
      receiverName: this.ms.receiver,
      text: this.inputContent,
      createdAt: new Date()
    }
    await this.auth.hubConnection?.invoke('SendPrivateMessage',messageModel)
      .then(() => this.inputContent='')
  }

}
