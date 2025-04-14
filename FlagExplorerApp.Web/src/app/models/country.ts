export interface Country {
  name: {
    common: string;
    official: string;
  };
  capital: string[];
  population: number;
  flags: {
    png: string;
    svg: string;
    alt?: string;
  };
  cca3: string; // ISO 3166-1 alpha-3 country code
}
