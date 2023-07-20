import {  Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UnreadedMessageModel } from 'src/app/models/unreaded-message-model';
import { UserModel } from 'src/app/models/user-model';
import { AuthService } from 'src/app/services/auth.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit,OnDestroy {

  user: UserModel | null = null
  users: Array<string> = []
  unreadedMessages: Array<UnreadedMessageModel> = []

  constructor(private router: Router,
              private auth: AuthService,
              private lss: LocalStorageService,
              private ms: MessageService){}

  async ngOnInit() {
    this.auth.user.subscribe({
      next: async um => {
        this.user = um
        if(this.user){
          await this.auth.getUsers()
            .then(res => {
              this.users = []
              res.forEach((userName: string) => {
                if(this.user?.userName !== userName){
                  this.users.push(userName)
                }
              });
            })
        }
      }
    })

    this.auth.unreadedMessages.subscribe({
      next: res => this.unreadedMessages = res
    })

    await this.auth.getCurrentUser()
  }

  getUnreadedMessage(senderName: string): number{
    return this.unreadedMessages.find(um => um.senderName === senderName)?.messageNumber as number
  }

  goToRegister(){
    this.lss.setMode('')
    this.router.navigate(['register'])
  }

  goToLogin(){
    this.lss.setMode('')
    this.router.navigate(['login'])
  }

  async goToChat(userName: string){

    await this.auth.hubConnection?.invoke('LeavePrivateMessage', this.lss.getReceiver())

    this.lss.setMode('private')
    this.ms.mode.next('private')
    this.lss.setReceiver(userName)

    if(this.router.url === '/chat'){
      this.auth.hubConnection?.off("ReceiveMessageFromUser")
      await this.ms.getPrivateMessages(this.user?.userName as string)
    }

    this.auth.getCurrentUser()
  }

  async logout(){
    this.lss.setMode('')
    this.router.navigate([''])
    await this.auth.logout()
  }

  async ngOnDestroy() {
    if(this.auth.hubConnection){
      this.auth.hubConnection.off
      await this.auth.hubConnection.stop()
    }
  }
}
