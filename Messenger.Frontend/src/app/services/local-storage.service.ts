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

  getReceiver(): string{
    return localStorage.getItem('receiver') ? localStorage.getItem('receiver') as string : ''
  }

  setReceiver(value: string){
    localStorage.setItem('receiver',value)
  }

  getMode(): string{
    return localStorage.getItem('mode') ? localStorage.getItem('mode') as string : ''
  }

  setMode(value: string){
    localStorage.setItem('mode',value)
  }

  getUser(): string{
    return localStorage.getItem('user') ? localStorage.getItem('user') as string : ''
  }

  setUser(value: string){
    localStorage.setItem('user',value)
  }
}
