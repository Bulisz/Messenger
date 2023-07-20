import { Component, OnInit } from '@angular/core';
import { MessageModel } from 'src/app/models/message-model';
import { UserModel } from 'src/app/models/user-model';
import { AuthService } from 'src/app/services/auth.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent implements OnInit {

  inputContent = ''
  connectedUsers = 0;
  mode: string | null = null


  constructor(private auth: AuthService, private lss: LocalStorageService, private ms: MessageService){}

  ngOnInit(): void {
    this.ms.mode.subscribe({
      next: um => this.mode = um
    })
    
    this.auth.connectedUsers.subscribe({next: res => this.connectedUsers = res})
  }

  async sendMessage(){
    let messageModel:MessageModel = {
      senderUserName: this.auth.user.value?.userName as string,
      receiverName: this.lss.getReceiver(),
      text: this.inputContent,
      createdAt: new Date()
    }
    await this.auth.hubConnection?.invoke('SendPrivateMessage',messageModel)
      .then(() => this.inputContent = '')
  }

}
