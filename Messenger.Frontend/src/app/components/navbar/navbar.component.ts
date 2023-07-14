import {  Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
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

  constructor(private router: Router, private auth: AuthService, private lss: LocalStorageService){}

  async ngOnInit() {
    this.auth.user.subscribe({
      next: async um => {
        this.user = um
        if(this.user){
          await this.auth.getUsers()
            .then(res => {
              res.forEach((userName: string) => {
                if(this.user?.userName !== userName){
                  this.users.push(userName)
                }
              });
            })
        }
      }
    })

    await this.auth.getCurrentUser()
  }

  goToRegister(){
    this.router.navigate(['register'])
  }

  goToLogin(){
    this.router.navigate(['login'])
  }

  goToChat(userName: string){
    this.lss.setReceiver(userName)
    //location.reload()
  }

  async logout(){
    await this.auth.logout()
    this.router.navigate([''])
  }

  async ngOnDestroy() {
    if(this.auth.hubConnection){
      await this.auth.hubConnection.stop()
    }
  }
}
