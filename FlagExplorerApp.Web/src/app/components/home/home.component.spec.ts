import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { HomeComponent } from './home.component';
import { CountryService } from '../../services/country.service';
import { Country } from '../../models/country';
import { By } from '@angular/platform-browser';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;
  let countryServiceMock: { getAllCountries: jest.Mock };

  const mockCountries: Country[] = [
    {
      name: {
        common: 'United States',
        official: 'United States of America',
      },
      capital: ['Washington D.C.'],
      population: 331002651,
      flags: {
        png: 'https://example.com/us.png',
        svg: 'https://example.com/us.svg',
      },
      cca3: 'USA',
    },
    {
      name: {
        common: 'Canada',
        official: 'Canada',
      },
      capital: ['Ottawa'],
      population: 37742154,
      flags: {
        png: 'https://example.com/ca.png',
        svg: 'https://example.com/ca.svg',
      },
      cca3: 'CAN',
    },
  ];

  beforeEach(async () => {
    countryServiceMock = {
      getAllCountries: jest.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [{ provide: CountryService, useValue: countryServiceMock }],
    }).compileComponents();

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    countryServiceMock.getAllCountries.mockReturnValue(of([]));
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should load countries on init', () => {
    countryServiceMock.getAllCountries.mockReturnValue(of(mockCountries));
    fixture.detectChanges();

    expect(countryServiceMock.getAllCountries).toHaveBeenCalled();
    expect(component.countries).toEqual(mockCountries);
    expect(component.loading).toBeFalsy();
    expect(component.error).toBeFalsy();
  });

  it('should handle error when loading countries fails', () => {
    const errorResponse = new Error('API Error');
    countryServiceMock.getAllCountries.mockReturnValue(
      throwError(() => errorResponse)
    );

    fixture.detectChanges();

    expect(countryServiceMock.getAllCountries).toHaveBeenCalled();
    expect(component.loading).toBeFalsy();
    expect(component.error).toBeTruthy();
  });

  it('should display the loading state initially', () => {
    // Don't call detectChanges yet to test initial state
    expect(component.loading).toBeTruthy();
  });

  it('should display countries in the grid after loading', () => {
    countryServiceMock.getAllCountries.mockReturnValue(of(mockCountries));
    fixture.detectChanges();

    const countryElements = fixture.debugElement.queryAll(
      By.css('.country-card')
    );
    expect(countryElements.length).toBe(2);

    const countryNames = fixture.debugElement.queryAll(
      By.css('.country-card h3')
    );
    expect(countryNames[0].nativeElement.textContent).toContain(
      'United States'
    );
    expect(countryNames[1].nativeElement.textContent).toContain('Canada');
  });

  it('should show error message when API call fails', () => {
    const errorResponse = new Error('API Error');
    countryServiceMock.getAllCountries.mockReturnValue(
      throwError(() => errorResponse)
    );
    fixture.detectChanges();

    const errorElement = fixture.debugElement.query(By.css('.error'));
    expect(errorElement).toBeTruthy();
    expect(errorElement.nativeElement.textContent).toContain(
      'Error loading countries'
    );
  });

  it('should retry loading countries when try again button is clicked', () => {
    // First attempt fails
    const errorResponse = new Error('API Error');
    countryServiceMock.getAllCountries.mockReturnValue(
      throwError(() => errorResponse)
    );
    fixture.detectChanges();

    // Verify first attempt state
    expect(component.error).toBeTruthy();
    expect(countryServiceMock.getAllCountries).toHaveBeenCalledTimes(1);

    // Reset component error state manually for the test
    component.error = false;

    // Set up mock for second attempt to succeed
    countryServiceMock.getAllCountries.mockReturnValue(of(mockCountries));

    // Click the try again button
    const tryAgainButton = fixture.debugElement.query(By.css('.error button'));
    tryAgainButton.nativeElement.click();
    fixture.detectChanges();

    // Verify second attempt
    expect(countryServiceMock.getAllCountries).toHaveBeenCalledTimes(2);
    expect(component.countries).toEqual(mockCountries);
    expect(component.loading).toBeFalsy();
  });
});
