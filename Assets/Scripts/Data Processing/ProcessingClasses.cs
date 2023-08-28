using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProcessingClasses : MonoBehaviour
{
  
}

class SmoothingFilter
{
    private const double M_PI = 3.141592653589793238462643383279502884;

    private float m_f_Xz_1; // x z-1 delay element
    private float m_f_Xz_2; // x z-2 delay element
    private float m_f_Yz_1; // y z-1 delay element
    private float m_f_Yz_2; // y z-2 delay element

    private double m_f_a0 = 0;
    private double m_f_a1 = 0;
    private double m_f_a2 = 0;
    private double m_f_b1 = 0;
    private double m_f_b2 = 0;

    public SmoothingFilter(float fc, float q, float fs)
    {
        FlushDelays();
        CalculateLPFCoeffs(fc, q, fs); // Initialize filter at startup
    }

    ~SmoothingFilter()
    {
        // Destructor
    }

    private void FlushDelays()
    {
        m_f_Xz_1 = 0;
        m_f_Xz_2 = 0;
        m_f_Yz_1 = 0;
        m_f_Yz_2 = 0;
    }

    public float DoFiltering(float f_xn)
    {
        // Calculate filter output
        float yn = (float)(m_f_a0 * f_xn + m_f_a1 * m_f_Xz_1 + m_f_a2 * m_f_Xz_2
            - m_f_b1 * m_f_Yz_1 - m_f_b2 * m_f_Yz_2);

        // Delay Shuffle
        m_f_Xz_2 = m_f_Xz_1;
        m_f_Xz_1 = f_xn;
        m_f_Yz_2 = m_f_Yz_1;
        m_f_Yz_1 = yn;

        // Check for NaN and return output
        if (float.IsNaN(yn)) yn = 0;
        return yn;
    }

    private void CalculateLPFCoeffs(float fCutoffFreq, float fQ, float fs)
    {
        // Use same terms as in book:
        float theta_c = 2.0f * (float)M_PI * fCutoffFreq / fs;
        float d = 1.0f / fQ;

        // Intermediate values
        float fBetaNumerator = 1.0f - ((d / 2.0f) * (float)Math.Sin(theta_c));
        float fBetaDenominator = 1.0f + ((d / 2.0f) * (float)Math.Sin(theta_c));

        // Beta
        float fBeta = 0.5f * (fBetaNumerator / fBetaDenominator);

        // Gamma
        float fGamma = (0.5f + fBeta) * (float)Math.Cos(theta_c);

        // Alpha
        float fAlpha = (0.5f + fBeta - fGamma) / 2.0f;

        // Coefficients
        m_f_a0 = (0.5f + fBeta - fGamma) / 2.0f;
        m_f_a1 = 0.5f + fBeta - fGamma;
        m_f_a2 = (0.5f + fBeta - fGamma) / 2.0f;
        m_f_b1 = -2 * fGamma;
        m_f_b2 = 2 * fBeta;
    }
}

class EnvelopeFollower
{
    private double attack = 0.0;
    private double release = 0.0;
    private double tc = -4.6051701859880913680359829093687;
    private double env_val = 0;

    public EnvelopeFollower(float releaseMs)
    {
        Set_TC(0.0f, releaseMs);
    }

    ~EnvelopeFollower()
    {
        // Destructor
    }

    private void Set_TC(float attackMs, float releaseMs)
    {
        attack = (attackMs > 0.0001f) ? Math.Exp(tc / (attackMs * 100 * 0.001)) : 0.0;
        release = (releaseMs > 0.0001f) ? Math.Exp(tc / (releaseMs * 100 * 0.001)) : 0.0;
    }

    public double GetEnvelope(double input)
    {
        if (input > env_val)
            env_val = input + attack * (env_val - input);
        else
            env_val = input + release * (env_val - input);

        return env_val; // Return the updated envelope value
    }
}


class Preprocessor
{
    // Processing Helper Elements
    private SmoothingFilter filt;

    // Fixed Parameters
    private float feat_MIN_Global;
    private float feat_MAX_Global;
    private float filt_Fc_LPF;

    // User-Modifiable Parameters
    private float feat_MIN_OfInterest;
    private float feat_MAX_OfInterest;
    private bool isInverted = false;

    // Helper variables
    private float inputVal = 0;
    private float outputVal = 0;
    private bool isInitialized = false;

    // Constructor
    public Preprocessor()
    {
        // Initialize filt as needed
        filt = new SmoothingFilter(0, 0, 0); // You should replace the parameters with appropriate values
    }

    // Destructor
    ~Preprocessor()
    {
        // Destructor
    }

    // Initialize - has to be called prior to use (!)
    public void Initialize(float featMinG, float featMaxG, float featMinI, float featMaxI, float filt_fc_L, bool isInv)
    {
        feat_MIN_Global = featMinG;
        feat_MAX_Global = featMaxG;
        feat_MIN_OfInterest = featMinI;
        feat_MAX_OfInterest = featMaxI;
        filt_Fc_LPF = filt_fc_L;
        filt = new SmoothingFilter(filt_Fc_LPF, 0.7f, 30); // Replace parameters as needed
        isInverted = isInv;
        isInitialized = true;
    }

    // Real-time setters for movement feature range, specify whether modifying min value or max value
    public void SetFeatRange_OfInterest(float val, bool isMin)
    {
        if (isMin)
            feat_MIN_OfInterest = val;
        else
            feat_MAX_OfInterest = val;
    }

    // Real-time setter for inversion flag
    public void SetIsInverted(bool isInv)
    {
        isInverted = isInv;
    }

    // Helper functions
    private float ApplyNormalize(float input)
    {
        if (input <= feat_MIN_OfInterest) return 0;
        if (input >= feat_MAX_OfInterest) return 1;
        return (input - feat_MIN_OfInterest) / (feat_MAX_OfInterest - feat_MIN_OfInterest);
    }

    // Movement feature is processed in every callback using this function
    public void Process(float input)
    {
        if (isInitialized)
        {
            inputVal = input;
            outputVal = filt.DoFiltering(inputVal);
            outputVal = ApplyNormalize(outputVal);
            if (isInverted) outputVal = 1 - outputVal;
        }
    }
}

class Postprocessor
{
    // Processing Helper Elements
    private EnvelopeFollower envFol;

    // Fixed Parameters
    private short mapFuncType = 1; // 1 = Exp, 2 = Sig, 3 = Lgt
    private bool isInverted = false;
    private float envRel_ms = 0;

    // User-Modifiable Parameters
    private float mapFunc_shape = 0.09f;

    // Helper Variables
    private float outputVal = 0;
    private bool isInitialized = false;

    // Constructor
    public Postprocessor()
    {
        // Initialize envFol as needed
        envFol = new EnvelopeFollower(0); // You should replace the parameter with an appropriate value
    }

    // Destructor
    ~Postprocessor()
    {
        // Destructor
    }

    // Initializer Function - has to be called for each instance prior to use (!)
    public void Initialize(short mapfn_type, bool isInv, float envReleaseMS)
    {
        mapFuncType = mapfn_type;
        isInverted = isInv;
        envRel_ms = envReleaseMS;
        envFol = new EnvelopeFollower(envRel_ms); // Replace the parameter with an appropriate value
        isInitialized = true;
    }

    // Real-time Setter - Mapping Function Shape
    public void SetMapFuncShape(float val)
    {
        mapFunc_shape = val;
    }

    // Apply nonlinear mapping function
    private float ApplyMapFunc(float input)
    {
        float output = 0;

        switch (mapFuncType)
        {
            case 1:
                output = (float)Math.Pow(input, mapFunc_shape);
                break;
            case 2:
                output = 1.0f / (1 + (float)Math.Exp(-mapFunc_shape * (10 * input - 5)));
                break;
            case 3:
                output = 0.5f + (float)(mapFunc_shape * Math.Log(input) / (1 - input * 1 / (16 + 8 * (mapFunc_shape - 1))));
                break;
            case 4:
                // If you still need to use sinPhase, declare it and update it here
                // float sinPhase = ...; // Initialize and update it appropriately
                output = (1 + (float)Math.Sin(input * mapFunc_shape)) * 0.5f;
                break;
        }

        if (float.IsNaN(output)) output = 0;
        output = Math.Clamp(output, 0.0f, 1.0f);
        return output;
    }

    // Preprocessor output is processed in every callback using this function
    public void Process(float input)
    {
        if (isInitialized)
        {
            double envelopeValue = envFol.GetEnvelope(input);
            outputVal = ApplyMapFunc((float)envelopeValue);
            if (isInverted) outputVal = 1 - outputVal;
        }
    }
}