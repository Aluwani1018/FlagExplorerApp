import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { CountryDetailComponent } from './components/country-detail/country-detail.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'country/:id', component: CountryDetailComponent },
  { path: '**', redirectTo: '' },
];
