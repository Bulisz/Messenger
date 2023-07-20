import { Component, Inject, OnInit, Renderer2 } from '@angular/core';
import { Router } from '@angular/router';
import { LoginRequestModel } from 'src/app/models/login-request-model';
import { AuthService } from 'src/app/services/auth.service';
import { CredentialResponse } from 'google-one-tap';
import { environment } from 'src/environments/environment';
import { GoogleLoginModel } from 'src/app/models/google-login-model';
import { DOCUMENT } from '@angular/common';
import { CreateGoogleUserModel } from 'src/app/models/create-google-user-model';
import { LocalStorageService } from 'src/app/services/local-storage.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent implements OnInit {

  userName = ''
  password = ''
  error = ''
  clientId = environment.clientId
  googleUser = false
  credential = ''

  constructor(private auth: AuthService, private router: Router, private _renderer2: Renderer2, @Inject(DOCUMENT) private _document: Document, private lss: LocalStorageService) { }

  ngOnInit() {
    // @ts-ignore
    window.onGoogleLibraryLoad = () => {
      // @ts-ignore
      google.accounts.id.initialize({
        client_id: this.clientId,
        callback: this.handleCredentialResponse.bind(this),
        auto_select: false,
        cancel_on_tap_outside: false,

      });
      // @ts-ignore
      google.accounts.id.renderButton(
        // @ts-ignore
        document.getElementById("buttonDiv"),
        { theme: "filled_blue", size: "large", width: "100%", shape: "rectangular", text: "signin", type: "standard" }
      );
    };
  }

  ngAfterViewInit() {
    const script1 = this._renderer2.createElement('script');
    script1.src = `https://accounts.google.com/gsi/client`;
    script1.async = `true`;
    script1.defer = `true`;
    this._renderer2.appendChild(this._document.body, script1);
  }

  async handleCredentialResponse(response: CredentialResponse) {
    this.lss.setMode('')
    let credential: GoogleLoginModel = { credential: response.credential }
    await this.auth.loginWithGoogle(credential)
      .then(() => {
        this.router.navigate([''])
      })
      .catch(err => {
        this.googleUser = true
        this.error = 'Írj be egy felhasználó nevet'
        this.credential = response.credential
        const element = document.getElementById('input1') as HTMLInputElement;
        setTimeout(() => element.focus(), 0);
      })
  }

  async submit() {
    this.lss.setMode('')
    if (!this.googleUser) {
      await this.login()
    } else {
      await this.googleRegister()
    }
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

  async googleRegister() {
    const regModel: CreateGoogleUserModel = {
      userName: this.userName,
      credential: this.credential
    }

    await this.auth.googleRegister(regModel)
      .then(() => {
        this.router.navigate([''])
      })
      .catch(err => this.error = err.error)
  }

  clearError() {
    this.error = ''
  }
}
