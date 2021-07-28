using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL;
using CSMSL.Proteomics;
using CSMSL.IO.Thermo;
using CSMSL.Chemistry;

namespace GenerateTheoreticalPeptideFragments
{
    public class MzFeature
    {
        private static Tolerance massTolerance = new Tolerance("-+20 PPM"); //change from -+ 10ppm
        public IMass feature;
        public int charge;
        private MzRange mzRange;
        public List<double> intensities;
        public List<int> spectrumNumbers;
        public List<double> spectrumTICs;

        public MzFeature(Peptide precursor, int charge)
        {
            this.feature = precursor;
            this.charge = charge;
            this.mzRange = new MzRange(this.feature.ToMz(this.charge), massTolerance);
            this.intensities = new List<double>();
            this.spectrumNumbers = new List<int>();
            this.spectrumTICs = new List<double>();
        }

        public MzFeature(Fragment fragment, int charge)
        {
            this.feature = fragment;
            this.charge = charge;
            this.mzRange = new MzRange(this.feature.ToMz(this.charge), massTolerance);
            this.intensities = new List<double>();
            this.spectrumNumbers = new List<int>();
            this.spectrumTICs = new List<double>();
        }

        public void MatchIntensities(ThermoSpectrum spectrum, int spectrumNumber)
        {
            var outputIntensity = 0.0;

            spectrum.TryGetIntensities(mzRange, out outputIntensity);

            intensities.Add(outputIntensity);
            spectrumNumbers.Add(spectrumNumber);
            spectrumTICs.Add(spectrum.TotalIonCurrent);
        }

        public double GetFeatureMz()
        {
            return mzRange.Mean;
        }
    }
}
