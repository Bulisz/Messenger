import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Observable,tap } from "rxjs";
import { AuthService } from "../services/auth.service";
import { Injectable } from "@angular/core";
import { LocalStorageService } from "../services/local-storage.service";

@Injectable({
  providedIn: 'root'
})

export class AuthInterceptor implements HttpInterceptor {

  constructor(private auth: AuthService, private lss: LocalStorageService){}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
  
      if(localStorage.getItem('accessToken')){
        const newRequest = req.clone({
          headers: req.headers.set('Authorization', `Bearer ${this.lss.getAccessToken()}`)
        })
        return next.handle(newRequest).pipe(
          tap({
            error: async err => {
              if(err.status === 401){
                return await this.handleRefresh(newRequest, next)
              }
            }
          })
          );
      }
      return next.handle(req)
    }

    
  async handleRefresh(req: HttpRequest<any>, next: HttpHandler): Promise<any> {
    return await this.auth.refresh()
      .then(() => {
        const newRequest = req.clone({
          headers: req.headers.set('Authorization', `Bearer ${this.lss.getAccessToken()}`)
        })
        return next.handle(newRequest)
      })
      .catch(err => console.log(err))
  }
  }