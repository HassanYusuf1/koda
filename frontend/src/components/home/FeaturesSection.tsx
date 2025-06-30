"use client";
import { Brain, BarChart3, Users, CalendarCheck } from "lucide-react";

const features = [
  {
    title: "Smart treningsplanlegging",
    icon: <CalendarCheck className="w-8 h-8 text-green-500" />,
    description: "Sett opp økter med mål, del dem med laget og følg opp fremdrift på tvers av sesongen.",
  },
  {
    title: "Data-drevet beslutningstaking",
    icon: <BarChart3 className="w-8 h-8 text-green-500" />,
    description: "Få innsikt i spillerprestasjoner og lagets utvikling gjennom lettfattelige rapporter.",
  },
  {
    title: "Individuell spilleroppfølging",
    icon: <Users className="w-8 h-8 text-green-500" />,
    description: "Se fremgang for hver enkelt spiller og gi personlig feedback – alt på ett sted.",
  },
  {
    title: "AI-assistert evaluering",
    icon: <Brain className="w-8 h-8 text-green-500" />,
    description: "NextPlay bruker kunstig intelligens for å hjelpe deg analysere data og tilpasse treningen.",
  },
];

const FeaturesSection = () => {
  return (
    <section className="bg-white py-20 px-6 md:px-16 text-gray-900">
      <div className="max-w-7xl mx-auto text-center">
        <h2 className="text-4xl font-bold mb-4">Hva tilbyr NextPlay?</h2>
        <p className="text-lg text-gray-600 mb-12">
          Alt du trenger for å analysere, planlegge og løfte laget ditt – smartere og mer effektivt.
        </p>
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8">
          {features.map((feature) => (
            <div
              key={feature.title}
              className="bg-gray-100 rounded-2xl shadow p-6 text-left transition hover:shadow-lg"
            >
              <div className="mb-4">{feature.icon}</div>
              <h3 className="text-xl font-semibold mb-2">{feature.title}</h3>
              <p className="text-gray-700 text-sm">{feature.description}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default FeaturesSection;
