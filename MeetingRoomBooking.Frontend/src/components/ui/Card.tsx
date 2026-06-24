import { type ReactNode } from 'react';
import { motion } from 'framer-motion';

interface CardProps {
  children: ReactNode;
  className?: string;
  hover?: boolean;
  onClick?: () => void;
}

export default function Card({
  children,
  className = '',
  hover = false,
  onClick,
}: CardProps) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
      className={`
        bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700
        shadow-sm shadow-gray-200/50 dark:shadow-none
        ${hover ? 'hover:shadow-md hover:border-gray-200 dark:hover:border-gray-600 hover:-translate-y-0.5 cursor-pointer' : ''}
        transition-all duration-300 ease-out
        ${className}
      `}
      onClick={onClick}
      whileHover={hover ? { scale: 1.01 } : undefined}
      whileTap={hover ? { scale: 0.99 } : undefined}
    >
      {children}
    </motion.div>
  );
}
