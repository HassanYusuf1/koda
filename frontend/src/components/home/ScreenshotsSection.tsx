"use client";

const screenshots = [
  {
    title: "Dashboard for trenere",
    description: "Få rask oversikt over spillere, økter og siste rapporter på én skjerm.",
    image: "https://unsplash.it/400/300?image=1001",
  },
  {
    title: "Spillerprofil",
    description: "Se fremgang og aktivitet for hver spiller, inkludert statistikk og notater.",
    image: "https://unsplash.it/400/300?image=1020",
  },
  {
    title: "Planlegging av økter",
    description: "Sett opp treningsøkter med mål, deltakere og målsettinger.",
    image: "https://unsplash.it/400/300?image=1063",
  },
];

const ScreenshotsSection = () => {
  return (
    <section className="bg-gray-100 py-20 px-6 md:px-16 text-gray-900">
      <div className="max-w-7xl mx-auto text-center">
        <h2 className="text-4xl font-bold mb-4">Se hvordan det ser ut</h2>
        <p className="text-lg text-gray-600 mb-12">
          NextPlay er designet for å være enkelt å bruke – men kraftig nok for alle nivåer.
        </p>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {screenshots.map((shot) => (
            <div key={shot.title} className="bg-white rounded-2xl shadow overflow-hidden">
              <img
                src={shot.image}
                alt={shot.title}
                className="w-full h-52 object-cover"
              />
              <div className="p-5 text-left">
                <h3 className="text-xl font-semibold">{shot.title}</h3>
                <p className="text-sm text-gray-600 mt-2">{shot.description}</p>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default ScreenshotsSection;
