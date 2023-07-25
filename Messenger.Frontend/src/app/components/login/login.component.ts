import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoginRequestModel } from 'src/app/models/login-request-model';
import { AuthService } from 'src/app/services/auth.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent {

  userName = ''
  password = ''
  error = ''

  constructor(private auth: AuthService, private router: Router, private lss: LocalStorageService) { }

  async submit() {
    this.lss.setMode('')
    await this.login()
  }

  async login() {
    const loginModel: LoginRequestModel = {
      userName: this.userName,
      password: this.password
    }
    await this.auth.login(loginModel)
      .then(() => {
        this.router.navigate([''])
      })
      .catch(err => this.error = err.error)
  }

  clearError() {
    this.error = ''
  }
}
