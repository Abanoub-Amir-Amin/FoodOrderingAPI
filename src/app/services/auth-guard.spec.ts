import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { authGuard } from './auth-guard';   // Use class-based guard if you renamed
import { AuthService } from './auth';
import { HttpClient } from '@angular/common/http';
import { of, throwError } from 'rxjs';

// Mock implementations
const mockRouter = {
  navigate: jasmine.createSpy('navigate'),
  createUrlTree: jasmine.createSpy('createUrlTree').and.returnValue({} as UrlTree),
};

const mockAuthService = {
  isLoggedIn: jasmine.createSpy('isLoggedIn'),
  getUserId: jasmine.createSpy('getUserId'),
  logout: jasmine.createSpy('logout'),
};

const mockHttpClient = {
  get: jasmine.createSpy('get'),
};

describe('AuthGuard', () => {
  let guard: authGuard;
  let router: Router;
  let authService: AuthService;
  let httpClient: HttpClient;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        authGuard,
        { provide: Router, useValue: mockRouter },
        { provide: AuthService, useValue: mockAuthService },
        { provide: HttpClient, useValue: mockHttpClient },
      ],
    });

    guard = TestBed.inject(authGuard);
    router = TestBed.inject(Router);
    authService = TestBed.inject(AuthService);
    httpClient = TestBed.inject(HttpClient);

    // Reset spies before each test
    mockRouter.navigate.calls.reset();
    mockRouter.createUrlTree.calls.reset();
    mockAuthService.isLoggedIn.calls.reset();
    mockAuthService.getUserId.calls.reset();
    mockAuthService.logout.calls.reset();
    mockHttpClient.get.calls.reset();
  });

  it('should deny access and redirect to login if user is not logged in', fakeAsync(() => {
    mockAuthService.isLoggedIn.and.returnValue(false);

    let result: boolean | UrlTree | undefined;
    guard.canActivate({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot).subscribe((res: boolean | UrlTree) => {
      result = res;
    });

    tick();

    expect(result).toEqual(mockRouter.createUrlTree.calls.mostRecent().returnValue);
    expect(mockRouter.navigate).not.toHaveBeenCalled();  // Preferred to return UrlTree, not navigate()
  }));

  it('should logout and redirect to login if no userId found', fakeAsync(() => {
    mockAuthService.isLoggedIn.and.returnValue(true);
    mockAuthService.getUserId.and.returnValue(null);

    let result: boolean | UrlTree | undefined;
    guard.canActivate({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot).subscribe((res: boolean | UrlTree) => {
      result = res;
    });

    tick();

    expect(mockAuthService.logout).toHaveBeenCalled();
    expect(result).toEqual(mockRouter.createUrlTree.calls.mostRecent().returnValue);
  }));

  it('should allow access if restaurant is active', fakeAsync(() => {
    mockAuthService.isLoggedIn.and.returnValue(true);
    mockAuthService.getUserId.and.returnValue('123');
    mockHttpClient.get.and.returnValue(of({ IsActive: true }));

    let result: boolean | UrlTree | undefined;
    guard.canActivate({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot).subscribe((res: boolean | UrlTree) => {
      result = res;
    });

    tick();

    expect(result).toBe(true);
  }));

  it('should redirect to action-pending if restaurant is not active', fakeAsync(() => {
    mockAuthService.isLoggedIn.and.returnValue(true);
    mockAuthService.getUserId.and.returnValue('123');
    mockHttpClient.get.and.returnValue(of({ IsActive: false }));

    let result: boolean | UrlTree | undefined;
    guard.canActivate({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot).subscribe((res: boolean | UrlTree) => {
      result = res;
    });

    tick();

    expect(result).toEqual(mockRouter.createUrlTree.calls.mostRecent().returnValue);
    expect(mockRouter.createUrlTree).toHaveBeenCalledWith(['/action-pending']);
  }));

  it('should redirect to login on http error', fakeAsync(() => {
    mockAuthService.isLoggedIn.and.returnValue(true);
    mockAuthService.getUserId.and.returnValue('123');
    mockHttpClient.get.and.returnValue(throwError(() => new Error('Http error')));

    let result: boolean | UrlTree | undefined;
    guard.canActivate({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot).subscribe((res: boolean | UrlTree) => {
      result = res;
    });

    tick();

    expect(result).toEqual(mockRouter.createUrlTree.calls.mostRecent().returnValue);
    expect(mockRouter.createUrlTree).toHaveBeenCalledWith(['/login']);
  }));
});
