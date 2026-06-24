declare module 'framer-motion' {
  import { ComponentType, ReactNode, HTMLAttributes } from 'react';

  export interface MotionProps {
    initial?: any;
    animate?: any;
    exit?: any;
    transition?: any;
    whileHover?: any;
    whileTap?: any;
    layoutId?: string;
    className?: string;
    style?: any;
    onClick?: () => void;
    children?: ReactNode;
  }

  export const motion: {
    div: ComponentType<MotionProps & HTMLAttributes<HTMLDivElement>>;
    [key: string]: ComponentType<any>;
  };

  export const AnimatePresence: ComponentType<{
    children?: ReactNode;
    mode?: 'wait' | 'sync' | 'popLayout';
  }>;
}
