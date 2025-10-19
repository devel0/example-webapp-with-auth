import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StackedSnackbar } from './stacked-snackbar';

describe('StackedSnackbar', () => {
  let component: StackedSnackbar;
  let fixture: ComponentFixture<StackedSnackbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StackedSnackbar]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StackedSnackbar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
