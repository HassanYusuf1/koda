"use client";

import Navbar from './Navbar';
import Footer from './Footer'

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
      <Footer/>

    </>
  );
}
