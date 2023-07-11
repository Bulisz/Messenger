import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {

  getAccessToken(): string{
    return localStorage.getItem('accessToken') ? localStorage.getItem('accessToken') as string : ''
  }

  setAccessToken(value: string){
    localStorage.setItem('accessToken',value)
  }
  
  clearAccessToken(){
    localStorage.removeItem('accessToken')
  }

  getRefreshToken(): string{
    return localStorage.getItem('refreshToken') ? localStorage.getItem('refreshToken') as string : ''
  }

  setRefreshToken(value: string){
    localStorage.setItem('refreshToken',value)
  }

  clearRefreshToken(){
    localStorage.removeItem('refreshToken')
  }
}
