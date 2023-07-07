import { Component, Inject, OnInit, Renderer2 } from '@angular/core';
import { Router } from '@angular/router';
import { LoginRequestModel } from 'src/app/models/login-request-model';
import { AuthService } from 'src/app/services/auth.service';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap';
import { environment } from 'src/environments/environment';
import { GoogleLoginModel } from 'src/app/models/google-login-model';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent implements OnInit {

  userName = ''
  password = ''
  error = ''
  private clientId = environment.clientId

  constructor(private auth: AuthService, private router: Router, private _renderer2: Renderer2, @Inject(DOCUMENT) private _document: Document){}

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
    let credential: GoogleLoginModel = {credential:response.credential}
    await this.auth.LoginWithGoogle(credential)
      .then(() => {
        this.router.navigate([''])
      })
      .catch(err => this.error = err.error)
}

  async submit() {
    const loginModel: LoginRequestModel ={
      userName: this.userName,
      password: this.password
    }
    await this.auth.login(loginModel)
      .then(() => {
        this.router.navigate([''])
      })
      .catch(err => this.error = err.error)
  }

  clearError(){
    this.error = ''
  }
}
