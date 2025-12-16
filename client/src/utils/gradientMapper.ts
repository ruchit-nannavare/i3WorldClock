import type { WeatherCondition, DayNightStatus } from '../types';

export interface GradientConfig {
  background: string;
  textColor: string;
}

export const getGradient = (
  condition: WeatherCondition,
  dayNight: DayNightStatus,
  temperature: number,
  isDarkMode: boolean = false
): GradientConfig => {
  // Dark mode overrides - gradient handled by styled-components
  if (isDarkMode) {
    return {
      background: 'transparent', // Not used, styled-components handles it
      textColor: '#ffffff',
    };
  }

  // Night always gets night gradient - darker
  if (dayNight === 'Night') {
    return {
      background:
        'linear-gradient(180deg, #0f0f1e 0%, #0d1628 50%, #0a2440 100%)',
      textColor: '#ffffff',
    };
  }

  // Hot weather (>30°C) - darker
  if (temperature > 30) {
    return {
      background:
        'linear-gradient(180deg, #E8857B 0%, #E85A78 50%, #E8899C 100%)',
      textColor: '#000000',
    };
  }

  // Weather-based gradients for daytime - all darker
  switch (condition) {
    case 'Clear':
    case 'PartlyCloudy':
      // Darker mint/teal to yellow to orange
      return {
        background:
          'linear-gradient(180deg, #C4E1B0 0%, #E5E598 20%, #EED87A 40%, #EFC460 60%, #EFAE4C 80%, #EF8F33 100%)',
        textColor: '#000000',
      };

    case 'Cloudy':
    case 'Fog':
      return {
        background:
          'linear-gradient(180deg, #D0D5DC 0%, #A8B5C6 50%, #8D9FB4 100%)',
        textColor: '#000000',
      };

    case 'Rain':
    case 'Thunderstorm':
      return {
        background:
          'linear-gradient(180deg, #566eda 0%, #4a5fc1 50%, #663b92 100%)',
        textColor: '#ffffff',
      };

    case 'Snow':
      return {
        background:
          'linear-gradient(180deg, #D8DFE5 0%, #C4D4E1 50%, #B5C8D8 100%)',
        textColor: '#000000',
      };

    default:
      return {
        background:
          'linear-gradient(180deg, #86D691 0%, #CDE26A 25%, #EED056 50%, #EFA337 75%, #EF8F33 100%)',
        textColor: '#000000',
      };
  }
};

export const getWeatherEmoji = (condition: WeatherCondition): string => {
  switch (condition) {
    case 'Clear':
      return '☀️';
    case 'PartlyCloudy':
      return '⛅';
    case 'Cloudy':
      return '☁️';
    case 'Rain':
      return '🌧️';
    case 'Snow':
      return '❄️';
    case 'Thunderstorm':
      return '⛈️';
    case 'Fog':
      return '🌫️';
    default:
      return '☀️';
  }
};

export const getDayNightEmoji = (dayNight: DayNightStatus): string => {
  return dayNight === 'Day' ? '☀️' : '🌙';
};
