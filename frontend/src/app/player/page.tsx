"use client";
import React from "react";
import Layout from "../../components/Layout";
import { Calendar, FileText, Star, LogOut } from "lucide-react";

const dummyPlayer = {
  fullName: "Ola Nordmann",
  email: "ola.nordmann@example.com",
  position: "Midtbane",
  team: "Rosenborg BK",
  stats: {
    sessionsAttended: 12,
    reportsSubmitted: 8,
    averageRating: 7.9,
  },
};

export default function PlayerDashboard() {
  return (
    <Layout>
      <div className="min-h-screen bg-gradient-to-br from-gray-950 via-gray-900 to-gray-800 text-white p-6">
        <div className="max-w-5xl mx-auto">
          {/* Profilheader */}
          <div className="flex flex-col sm:flex-row items-center sm:items-start sm:justify-between gap-6 mb-10">
            <div className="flex items-center gap-4">
              <div className="w-20 h-20 rounded-full bg-indigo-600 flex items-center justify-center text-3xl font-bold shadow-lg">
                {dummyPlayer.fullName.charAt(0)}
              </div>
              <div>
                <h1 className="text-3xl font-semibold">{dummyPlayer.fullName}</h1>
                <p className="text-gray-400">{dummyPlayer.email}</p>
                <p className="text-sm text-gray-500 mt-1">
                  {dummyPlayer.position} | {dummyPlayer.team}
                </p>
              </div>
            </div>
            <button className="flex items-center gap-2 bg-red-600 hover:bg-red-500 px-4 py-2 rounded-xl transition text-sm font-medium">
              <LogOut size={18} />
              Logg ut
            </button>
          </div>

          {/* Stat Cards */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-10">
            <div className="bg-gray-800 border border-gray-700 rounded-2xl p-5 shadow hover:shadow-indigo-500/10 transition">
              <div className="flex items-center gap-3 mb-2 text-indigo-400">
                <Calendar size={20} />
                <p className="text-sm font-medium">Treningsøkter</p>
              </div>
              <h2 className="text-3xl font-bold">{dummyPlayer.stats.sessionsAttended}</h2>
            </div>
            <div className="bg-gray-800 border border-gray-700 rounded-2xl p-5 shadow hover:shadow-indigo-500/10 transition">
              <div className="flex items-center gap-3 mb-2 text-green-400">
                <FileText size={20} />
                <p className="text-sm font-medium">Rapporter levert</p>
              </div>
              <h2 className="text-3xl font-bold">{dummyPlayer.stats.reportsSubmitted}</h2>
            </div>
            <div className="bg-gray-800 border border-gray-700 rounded-2xl p-5 shadow hover:shadow-indigo-500/10 transition">
              <div className="flex items-center gap-3 mb-2 text-yellow-400">
                <Star size={20} />
                <p className="text-sm font-medium">Snittvurdering</p>
              </div>
              <h2 className="text-3xl font-bold">{dummyPlayer.stats.averageRating}</h2>
            </div>
          </div>

          {/* Navigasjon */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <button className="w-full bg-indigo-600 hover:bg-indigo-500 text-white py-3 rounded-xl transition font-medium shadow">
              Se rapporter
            </button>
            <button className="w-full bg-indigo-600 hover:bg-indigo-500 text-white py-3 rounded-xl transition font-medium shadow">
              Mine økter
            </button>
          </div>
        </div>
      </div>
    </Layout>
  );
}
