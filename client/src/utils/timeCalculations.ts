export const getLocalTime = (utcTime: Date, utcOffsetSeconds: number): Date => {
  const ms = utcTime.getTime() + utcOffsetSeconds * 1000;
  return new Date(ms);
};

export const isDayTime = (localHour: number): boolean => {
  return localHour >= 6 && localHour < 18;
};

export const formatTime = (date: Date, is24Hour: boolean): string => {
  const hours = date.getUTCHours();
  const minutes = date.getUTCMinutes();
  const seconds = date.getUTCSeconds();

  if (is24Hour) {
    return `${hours.toString().padStart(2, '0')}:${minutes
      .toString()
      .padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
  }

  const period = hours >= 12 ? 'PM' : 'AM';
  const displayHours = hours % 12 || 12;
  return `${displayHours.toString().padStart(2, '0')}:${minutes
    .toString()
    .padStart(2, '0')}:${seconds.toString().padStart(2, '0')} ${period}`;
};

export const formatTimeShort = (date: Date, is24Hour: boolean): string => {
  const hours = date.getUTCHours();
  const minutes = date.getUTCMinutes();

  if (is24Hour) {
    return `${hours.toString().padStart(2, '0')}:${minutes
      .toString()
      .padStart(2, '0')}`;
  }

  const period = hours >= 12 ? 'PM' : 'AM';
  const displayHours = hours % 12 || 12;
  return `${displayHours.toString().padStart(2, '0')}:${minutes
    .toString()
    .padStart(2, '0')} ${period}`;
};

export const calculateSunlightDuration = (
  sunrise: string,
  sunset: string
): string => {
  const [sunriseHours, sunriseMinutes] = sunrise.split(':').map(Number);
  const [sunsetHours, sunsetMinutes] = sunset.split(':').map(Number);

  const sunriseTotal = sunriseHours * 60 + sunriseMinutes;
  const sunsetTotal = sunsetHours * 60 + sunsetMinutes;

  const durationMinutes = sunsetTotal - sunriseTotal;
  const hours = Math.floor(durationMinutes / 60);
  const minutes = durationMinutes % 60;

  return `${hours}h ${minutes.toString().padStart(2, '0')}m`;
};
