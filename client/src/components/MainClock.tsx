import { useMemo } from 'react';
import { getLocalTime, formatTime } from '../utils/timeCalculations';
import './MainClock.css';

interface MainClockProps {
  utcTime: Date;
  utcOffsetSeconds: number;
  is24Hour: boolean;
}

export function MainClock({ utcTime, utcOffsetSeconds, is24Hour }: MainClockProps) {
  const localTime = useMemo(() => {
    return getLocalTime(utcTime, utcOffsetSeconds);
  }, [utcTime, utcOffsetSeconds]);

  const timeString = useMemo(() => {
    return formatTime(localTime, is24Hour);
  }, [localTime, is24Hour]);

  return (
    <div className="main-clock">
      <span className={`main-clock-time ${is24Hour ? 'clock-24h' : 'clock-12h'}`}>
        {timeString}
      </span>
    </div>
  );
}
