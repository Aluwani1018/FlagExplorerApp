import { TestBed } from '@angular/core/testing';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { CountryService } from './country.service';
import { Country } from '../models/country';

describe('CountryService', () => {
  let service: CountryService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [CountryService],
    });
    service = TestBed.inject(CountryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all countries', () => {
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
    ];

    service.getAllCountries().subscribe((countries) => {
      expect(countries).toEqual(mockCountries);
      expect(countries.length).toBe(1);
    });

    const req = httpMock.expectOne('https://restcountries.com/v3.1/all');
    expect(req.request.method).toBe('GET');
    req.flush(mockCountries);
  });

  it('should fetch a country by code', () => {
    const mockCountry: Country[] = [
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
    ];

    service.getCountryByCode('USA').subscribe((country) => {
      expect(country).toEqual(mockCountry);
    });

    const req = httpMock.expectOne('https://restcountries.com/v3.1/alpha/USA');
    expect(req.request.method).toBe('GET');
    req.flush(mockCountry);
  });

  it('should handle errors when API fails', () => {
    service.getAllCountries().subscribe({
      next: () => fail('Expected an error, not countries'),
      error: (error) => {
        expect(error instanceof Error).toBeTruthy();
        expect(error.message).toContain('Something went wrong');
      },
    });

    const req = httpMock.expectOne('https://restcountries.com/v3.1/all');
    req.flush('Error fetching countries', {
      status: 500,
      statusText: 'Server Error',
    });
  });
});
