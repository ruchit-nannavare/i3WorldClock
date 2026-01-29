/**
 * Map projection utilities for converting lat/long to pixel coordinates
 * Uses Mercator projection via react-simple-maps
 */

/**
 * Validates if coordinates are within valid ranges
 */
export function isValidCoordinate(latitude: number, longitude: number): boolean {
  return (
    latitude >= -90 &&
    latitude <= 90 &&
    longitude >= -180 &&
    longitude <= 180 &&
    !isNaN(latitude) &&
    !isNaN(longitude)
  );
}

/**
 * Helper to clamp values within a range
 */
export function clamp(value: number, min: number, max: number): number {
  return Math.min(Math.max(value, min), max);
}
