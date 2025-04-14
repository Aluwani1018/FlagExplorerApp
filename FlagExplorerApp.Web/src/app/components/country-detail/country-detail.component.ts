import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CountryService } from '../../services/country.service';
import { Country } from '../../models/country';

@Component({
  selector: 'app-country-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './country-detail.component.html',
  styleUrls: ['./country-detail.component.scss'],
})
export class CountryDetailComponent implements OnInit {
  country: Country | null = null;
  loading = true;
  error = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private countryService: CountryService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const countryCode = params.get('id');
      if (countryCode) {
        this.loadCountry(countryCode);
      } else {
        this.router.navigate(['/']);
      }
    });
  }

  loadCountry(code: string): void {
    this.countryService.getCountryByCode(code).subscribe({
      next: (data) => {
        if (data && data.length > 0) {
          this.country = data[0];
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching country details:', error);
        this.error = true;
        this.loading = false;
      },
    });
  }

  goBack(): void {
    this.router.navigate(['/']);
  }
}
