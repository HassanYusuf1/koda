// i18n.ts

export const locales = ['no', 'en'] as const;
export const defaultLocale = 'no';

export type Locale = (typeof locales)[number];
