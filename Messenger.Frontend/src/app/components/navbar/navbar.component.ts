import {  Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserModel } from 'src/app/models/user-model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {

  user: UserModel | null = null

  constructor(private router: Router, private auth: AuthService){}

  async ngOnInit() {
    this.auth.user.subscribe({
      next: um => this.user = um
    })

    if(localStorage.getItem('accessToken')){
      await this.auth.getCurrentUser()
    }
  }

  goToRegister(){
    this.router.navigate(['register'])
  }

  goToLogin(){
    this.router.navigate(['login'])
  }

  async logout(){
    await this.auth.logout()
    this.router.navigate([''])
  }
}
