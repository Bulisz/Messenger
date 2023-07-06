import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Observable,tap } from "rxjs";
import { AuthService } from "../services/auth.service";
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root'
})

export class AuthInterceptor implements HttpInterceptor {

  constructor(private auth: AuthService){}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
  
      if(localStorage.getItem('accessToken')){
        const newRequest = req.clone({
          headers: req.headers.set('Authorization', `Bearer ${localStorage.getItem('accessToken')}`)
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
      .then(td => {
        const newRequest = req.clone({
          headers: req.headers.set('Authorization', `Bearer ${td.accessToken.value}`)
        })
        return next.handle(newRequest)
      })
  }
  }