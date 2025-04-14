import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { of, throwError } from 'rxjs';
import { CountryDetailComponent } from './country-detail.component';
import { CountryService } from '../../services/country.service';
import { Country } from '../../models/country';
import { By } from '@angular/platform-browser';

describe('CountryDetailComponent', () => {
  let component: CountryDetailComponent;
  let fixture: ComponentFixture<CountryDetailComponent>;
  let countryServiceMock: { getCountryByCode: jest.Mock };
  let routerMock: { navigate: jest.Mock };

  const mockCountry: Country = {
    name: {
      common: 'Germany',
      official: 'Federal Republic of Germany',
    },
    capital: ['Berlin'],
    population: 83190556,
    flags: {
      png: 'https://example.com/de.png',
      svg: 'https://example.com/de.svg',
      alt: 'Flag of Germany',
    },
    cca3: 'DEU',
  };

  beforeEach(async () => {
    countryServiceMock = {
      getCountryByCode: jest.fn(),
    };
    routerMock = {
      navigate: jest.fn(),
    };

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        { provide: CountryService, useValue: countryServiceMock },
        { provide: Router, useValue: routerMock },
        {
          provide: ActivatedRoute,
          useValue: {
            paramMap: of(convertToParamMap({ id: 'DEU' })),
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CountryDetailComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    countryServiceMock.getCountryByCode.mockReturnValue(of([mockCountry]));
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should load country details based on route parameter', () => {
    countryServiceMock.getCountryByCode.mockReturnValue(of([mockCountry]));
    fixture.detectChanges();

    expect(countryServiceMock.getCountryByCode).toHaveBeenCalledWith('DEU');
    expect(component.country).toEqual(mockCountry);
    expect(component.loading).toBeFalsy();
    expect(component.error).toBeFalsy();
  });

  it('should handle error when loading country fails', () => {
    const errorResponse = new Error('API Error');
    countryServiceMock.getCountryByCode.mockReturnValue(
      throwError(() => errorResponse)
    );

    fixture.detectChanges();

    expect(countryServiceMock.getCountryByCode).toHaveBeenCalledWith('DEU');
    expect(component.loading).toBeFalsy();
    expect(component.error).toBeTruthy();
  });

  it('should display country details when loaded successfully', () => {
    countryServiceMock.getCountryByCode.mockReturnValue(of([mockCountry]));
    fixture.detectChanges();

    const countryName = fixture.debugElement.query(By.css('.country-title h1'));
    const countryOfficialName = fixture.debugElement.query(
      By.css('.country-title h3')
    );
    const countryCapital = fixture.debugElement.query(
      By.css('.info-card:nth-child(1) p')
    );
    const countryPopulation = fixture.debugElement.query(
      By.css('.info-card:nth-child(2) p')
    );

    expect(countryName.nativeElement.textContent).toContain('Germany');
    expect(countryOfficialName.nativeElement.textContent).toContain(
      'Federal Republic of Germany'
    );
    expect(countryCapital.nativeElement.textContent).toContain('Berlin');
    expect(countryPopulation.nativeElement.textContent).toContain('83,190,556');
  });

  it('should display error message when API call fails', () => {
    countryServiceMock.getCountryByCode.mockReturnValue(
      throwError(() => new Error('API Error'))
    );
    fixture.detectChanges();

    const errorElement = fixture.debugElement.query(By.css('.error'));
    expect(errorElement).toBeTruthy();
    expect(errorElement.nativeElement.textContent).toContain(
      'Error loading country details'
    );
  });

  it('should navigate back when back button is clicked', () => {
    countryServiceMock.getCountryByCode.mockReturnValue(of([mockCountry]));
    fixture.detectChanges();

    const backButton = fixture.debugElement.query(By.css('.back-button'));
    backButton.triggerEventHandler('click', null);

    expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
  });

  // Create a separate test for the missing country code scenario
  it('should navigate back when country code is missing', async () => {
    // Create a new test module with empty route params
    await TestBed.resetTestingModule()
      .configureTestingModule({
        imports: [RouterTestingModule],
        providers: [
          { provide: CountryService, useValue: countryServiceMock },
          { provide: Router, useValue: routerMock },
          {
            provide: ActivatedRoute,
            useValue: {
              paramMap: of(convertToParamMap({})),
            },
          },
        ],
      })
      .compileComponents();

    const emptyParamFixture = TestBed.createComponent(CountryDetailComponent);
    emptyParamFixture.detectChanges();

    expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
  });
});
