import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from '../services/auth.service';
import {jwtDecode} from "jwt-decode"

export const authGuard: CanActivateFn = (route, state) => {
  const cookieService = inject(CookieService)
  const authService = inject(AuthService)
  const router = inject(Router)
  const user = authService.getUser()

  // check for JWT Token  
  let token = cookieService.get("Authorization")

  if (token && user) {
    token = token.replace('Bearer ', "")
    const decodedToken: any = jwtDecode(token)

    const expiration = decodedToken.exp * 1000
    const currentTime = new Date().getTime()
    
    // check expiration date
    if(expiration < currentTime) {
      authService.logout();
      return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url }})
    } else {

      // check if roles has auth
      if (user.roles.includes('Writer')) {
        return true
      } 
      // return false
      else {
        alert('Unauthorized')
        return false
      }
    }
  } else {
    // logout
    authService.logout();
    return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url }})
  }
  
};
