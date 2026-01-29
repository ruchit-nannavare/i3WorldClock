import { useState, useCallback, useEffect } from 'react';
import { useLocalStorage } from './useLocalStorage';

interface MapViewState {
  isMapExpanded: boolean;
  isAnimating: boolean;
  toggleMapView: () => void;
}

export const ANIMATION_DURATION = 400; // ms

export function useMapView(): MapViewState {
  const [isMapExpanded, setIsMapExpanded] = useLocalStorage('i3worldclock-mapview', false);
  const [isAnimating, setIsAnimating] = useState(false);

  const toggleMapView = useCallback(() => {
    setIsAnimating(true);
    setIsMapExpanded((prev) => !prev);

    // Reset animation state after transition completes
    setTimeout(() => {
      setIsAnimating(false);
    }, ANIMATION_DURATION);
  }, [setIsMapExpanded]);

  // Keyboard navigation: Escape key to collapse map
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && isMapExpanded) {
        toggleMapView();
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [isMapExpanded, toggleMapView]);

  return {
    isMapExpanded,
    isAnimating,
    toggleMapView,
  };
}
