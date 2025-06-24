"use client";

import Navbar from './Navbar';

type LayoutProps = {
  children: React.ReactNode;
};

export default function Layout({ children }: LayoutProps) {
  return (
    <>
      <Navbar />
      <main style={{ padding: '2rem' }}>
        {children}
      </main>
    </>
  );
}
