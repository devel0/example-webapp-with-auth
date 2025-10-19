import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Some } from './some';

describe('Some', () => {
  let component: Some;
  let fixture: ComponentFixture<Some>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Some]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Some);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
