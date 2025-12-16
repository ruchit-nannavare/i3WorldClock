export interface City {
  id: string;
  name: string;
  country: string;
  countryCode: string;
  latitude: number;
  longitude: number;
  utcOffsetSeconds: number;
  utcOffsetDisplay: string;
}

export type WeatherCondition =
  | 'Clear'
  | 'PartlyCloudy'
  | 'Cloudy'
  | 'Rain'
  | 'Snow'
  | 'Thunderstorm'
  | 'Fog';

export type DayNightStatus = 'Day' | 'Night';

export interface Weather {
  temperature: number;
  condition: WeatherCondition;
  dayNight: DayNightStatus;
  sunrise: string;
  sunset: string;
}

export interface UtcTime {
  utcTime: string;
  unixTimestamp: number;
}

export interface TimeFormat {
  is24Hour: boolean;
}

export interface AppSettings {
  timeFormat: TimeFormat;
  primaryCityId: string | null;
  savedCities: City[];
}
