import type { ReactNode } from 'react';
import styled from 'styled-components';
import type { GradientConfig } from '../utils/gradientMapper';
import './GradientBackground.css';

interface GradientBackgroundProps {
  gradient: GradientConfig;
  children: ReactNode;
  isDarkMode?: boolean;
}

const StyledGradientBackground = styled.div<{ $textColor: string; $isDarkMode: boolean }>`
  min-height: 100vh;
  width: 100%;
  position: relative;
  color: ${props => props.$textColor};
  transition: all 0.5s ease;
  
  /* Base - light mode: mostly white/cream with peachy undertone, dark mode: deep purple */
  background: ${props => props.$isDarkMode ? '#1a0f2e' : '#FAF8F5'};
  
  /* Top-left accent - icy blue (light) / darker purple (dark) */
  &::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: ${props => props.$isDarkMode ? `radial-gradient(
      ellipse 70% 50% at 15% 15%,
      rgba(60, 30, 80, 0.5) 0%,
      rgba(50, 25, 70, 0.3) 25%,
      rgba(40, 20, 60, 0.15) 45%,
      transparent 65%
    )` : `radial-gradient(
      ellipse 70% 50% at 15% 15%,
      rgba(200, 230, 245, 0.3) 0%,
      rgba(210, 235, 250, 0.18) 25%,
      rgba(220, 240, 252, 0.08) 45%,
      transparent 65%
    )`};
    pointer-events: none;
  }
  
  /* Multiple radial overlays for depth and warmth */
  &::after {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: ${props => props.$isDarkMode ? `
      /* Main peachy glow - off-center to the right */
      radial-gradient(
        ellipse 65% 45% at 60% 35%,
        rgba(255, 180, 140, 0.25) 0%,
        rgba(255, 160, 120, 0.15) 20%,
        rgba(255, 140, 100, 0.08) 40%,
        transparent 65%
      ),
      /* Bottom peachy warmth with purple undertone */
      radial-gradient(
        ellipse 100% 45% at 50% 100%,
        rgba(200, 120, 160, 0.3) 0%,
        rgba(180, 100, 140, 0.2) 20%,
        rgba(160, 80, 120, 0.12) 40%,
        rgba(140, 60, 100, 0.06) 60%,
        transparent 75%
      ),
      /* Right side deep purple */
      radial-gradient(
        ellipse 55% 70% at 88% 45%,
        rgba(80, 40, 120, 0.35) 0%,
        rgba(70, 35, 100, 0.2) 30%,
        rgba(60, 30, 80, 0.1) 50%,
        transparent 70%
      ),
      /* Subtle peachy-purple tint overall */
      radial-gradient(
        ellipse 120% 100% at 50% 50%,
        rgba(120, 60, 100, 0.15) 0%,
        rgba(100, 50, 80, 0.08) 50%,
        transparent 100%
      )
    ` : `
      /* Main yellow glow - off-center to the right */
      radial-gradient(
        ellipse 65% 45% at 60% 35%,
        rgba(255, 245, 190, 0.55) 0%,
        rgba(255, 240, 170, 0.35) 20%,
        rgba(255, 235, 150, 0.18) 40%,
        transparent 65%
      ),
      /* Bottom peachy warmth - stronger and more orange */
      radial-gradient(
        ellipse 100% 45% at 50% 100%,
        rgba(255, 190, 130, 0.35) 0%,
        rgba(255, 200, 145, 0.25) 20%,
        rgba(255, 210, 160, 0.15) 40%,
        rgba(255, 220, 180, 0.08) 60%,
        transparent 75%
      ),
      /* Right side stronger icy blue */
      radial-gradient(
        ellipse 55% 70% at 88% 45%,
        rgba(180, 220, 245, 0.25) 0%,
        rgba(195, 230, 250, 0.15) 30%,
        rgba(210, 235, 252, 0.08) 50%,
        transparent 70%
      ),
      /* Subtle peachy tint overall */
      radial-gradient(
        ellipse 120% 100% at 50% 50%,
        rgba(255, 235, 215, 0.12) 0%,
        rgba(255, 240, 225, 0.06) 50%,
        transparent 100%
      )
    `};
    pointer-events: none;
  }
  
  /* Content wrapper */
  > * {
    position: relative;
    z-index: 1;
  }
`;

export function GradientBackground({ gradient, children, isDarkMode = false }: GradientBackgroundProps) {
  return (
    <StyledGradientBackground $textColor={gradient.textColor} $isDarkMode={isDarkMode}>
      {children}
    </StyledGradientBackground>
  );
}
