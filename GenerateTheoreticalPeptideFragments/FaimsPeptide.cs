using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL;
using CSMSL.Proteomics;
using CSMSL.IO.Thermo;

namespace GenerateTheoreticalPeptideFragments
{
    public class FaimsPeptide
    {
        public Peptide peptide;
        public int precursorCharge;
        public int maxFragmentCharge;
        public List<MzFeature> mzFeatures = new List<MzFeature>();

        // default arguments: precursor charge = +2, no PTMs. Provide different charges/mods/mod locations if that is not the case.
        public FaimsPeptide(string sequence, int precursorCharge = 2, Modification[] mods = null, int[] modLocation = null)
        {
            this.peptide = new Peptide(sequence);
            this.precursorCharge = precursorCharge;
            this.maxFragmentCharge = precursorCharge - 1;
            this.mzFeatures = new List<MzFeature>();

            // if mods and locations are provided
            if ((mods != null && mods.Length != 0) && (modLocation != null && modLocation.Length != 0))
            {
                if (mods.Length != modLocation.Length)
                {
                    throw new ArgumentException("The number of modifications and modification locations provided to the FaimsPeptide constructor are not equal.");
                }
                else
                {
                    for (var i = 0; i < mods.Length; i++)
                    {
                        this.peptide.AddModification(mods[i], modLocation[i]);
                    }
                }
            }

            // now generate all the theoretical peptide fragments and the intact precursor
            GenerateMzFeatures();
        }

        // use this constructur if you're building the Peptide object yourself. Anji, you'll be using this one.
        public FaimsPeptide(Peptide peptide, int precursorCharge = 2)
        {
            this.peptide = peptide;
            this.precursorCharge = precursorCharge;
            this.maxFragmentCharge = precursorCharge - 1;
            this.mzFeatures = new List<MzFeature>();

            // now generate all the theoretical peptide fragments and the intact precursor
            GenerateMzFeatures();
        }

        private void GenerateMzFeatures()
        {
            // add precursor to mzFeatures in case Anji is interested in tracking it
            this.mzFeatures.Add(new MzFeature(this.peptide, this.precursorCharge));

            // fragment peptide in-silico to determine all theoretical peptide fragments;
            var peptideFragments = peptide.Fragment(FragmentTypes.b | FragmentTypes.y).ToList();

            foreach (var fragment in peptideFragments)
            {
                for (var charge = maxFragmentCharge; charge > 0; charge--)
                {
                    this.mzFeatures.Add(new MzFeature(fragment, charge));
                }
            }
        }

        public void ProcessSpectrum(ThermoRawFile rawfile, int spectrumNumber)
        {
            foreach (var mzFeature in mzFeatures)
            {
                mzFeature.MatchIntensities(rawfile.GetSpectrum(spectrumNumber), spectrumNumber);
            }
        }

        public double SumFeatureIntensityAcrossScans(MzFeature mzFeature)
        {
            return mzFeature.intensities.Sum();
        }
    }
}
