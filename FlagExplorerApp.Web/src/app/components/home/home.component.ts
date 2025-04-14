import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CountryService } from '../../services/country.service';
import { Country } from '../../models/country';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  countries: Country[] = [];
  loading = true;
  error = false;

  constructor(private countryService: CountryService) {}

  ngOnInit(): void {
    this.loadCountries();
  }

  loadCountries(): void {
    this.countryService.getAllCountries().subscribe({
      next: (data) => {
        this.countries = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching countries:', error);
        this.error = true;
        this.loading = false;
      },
    });
  }
}
