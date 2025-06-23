import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Home } from './home';
import { ActivatedRoute } from '@angular/router';


const mockActivatedRoute = {
  snapshot:{
    paramMap:{
      get:(key:string)=>{
        return key == 'un' ? 'Nithish' : null;
      }
    }
  }
}

describe('Home', () => {
  let component: Home;
  let fixture: ComponentFixture<Home>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Home],
      providers:[{provide:ActivatedRoute,useValue:mockActivatedRoute}]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Home);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should read uname from route params', () => {
    expect(component.uname).toBe('Nithish');
  });
});
