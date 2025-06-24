"use client";

import Link from "next/link";
import React from "react";

const Navbar: React.FC = () => {
  return (
    <nav className="bg-blue-900 text-white p-5 flex justify-between items-center shadow-md">
      <div className="text-3xl font-extrabold tracking-wide">
        <Link href="/">⚽ Fotballlaget</Link>
      </div>
      <ul className="flex text-lg font-medium">
        <li className="mr-10">
          <Link href="/" className="hover:text-gray-300 transition-colors duration-200">
            Hjem
          </Link>
        </li>
        <li className="mr-10">
          <Link href="/players" className="hover:text-gray-300 transition-colors duration-200">
            Spillere
          </Link>
        </li>
        <li className="mr-10">
          <Link href="/sessions" className="hover:text-gray-300 transition-colors duration-200">
            Økter
          </Link>
        </li>
        <li className="mr-10">
          <Link href="/reports" className="hover:text-gray-300 transition-colors duration-200">
            Rapporter
          </Link>
        </li>
        <li className="mr-10">
          <Link href="/profile" className="hover:text-gray-300 transition-colors duration-200">
            Min Profil
          </Link>
        </li>
        <li>
          <Link href="/login" className="hover:text-gray-300 transition-colors duration-200">
            Logg Inn
          </Link>
        </li>
      </ul>
    </nav>
  );
};

export default Navbar;
