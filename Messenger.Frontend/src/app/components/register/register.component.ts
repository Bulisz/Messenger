import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CreateUserModel } from 'src/app/models/create-user-model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {

  userName = ''
  password = ''
  confirmPassword = ''
  email = ''
  error = ''

  constructor(private auth: AuthService, private router: Router){}

  submit() {
    const registerModel: CreateUserModel ={
      userName: this.userName,
      password: this.password,
      confirmPassword: this.confirmPassword,
      email: this.email
    }
    this.auth.registerNewAccout(registerModel)
      .then(() => {
        this.router.navigate(['login'])
      })
      .catch(err => {
        if(err.error.errors){
          for(const key of Object.keys(err.error.errors)){
            this.error += err.error.errors[key]
          }
        } else {
          this.error = err.error
        }
      })
  }

  clearError(){
    this.error = ''
  }

}
