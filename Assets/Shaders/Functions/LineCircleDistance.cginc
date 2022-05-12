
float LineCircleDistance(float3 lineOrigin, float3 lineDirection, float3 circleCenter, float3 circleNormal, float3 circleRadius)
{
    Result result{};
    float3 vzero = 0;

    float3 D = lineOrigin - circleCenter;
    float3 NxM = cross(circleNormal, lineDirection);
    float3 NxD = cross(circleNormal, D);
    float t = 0.0;

    if (NxM != vzero)
    {
        if (NxD != vzero)
        {
            float NdM = dot(circleNormal, lineDirection);
            if (NdM != 0.0)
            {
                // H(t) = (a*t^2 + 2*b*t + c)*(t + d)^2
                //        - r^2*(a*t + b)^2
                //      = h0 + h1*t + h2*t^2 + h3*t^3 + h4*t^4
                float a = dot(NxM, NxM), b = dot(NxM, NxD);
                float c = dot(NxD, NxD), d = dot(lineDirection, D);
                float rsqr = circleRadius * circleRadius;
                float asqr = a * a, bsqr = b * b, dsqr = d * d;
                float h0 = c * dsqr - bsqr * rsqr;
                float h1 = 2.0 * (c * d + b * dsqr - a * b * rsqr);
                float h2 = c + 4.0 * b * d + a * dsqr - asqr * rsqr;
                float h3 = 2.0 * (b + a * d);
                float h4 = a;

                std::map<float, int32_t> rmMap{};
                RootsPolynomial<float>::template SolveQuartic<float>(h0, h1, h2, h3, h4, rmMap);
                std::array<ClosestInfo, 4> candidates{};
                size_t numRoots = 0;
                for (auto const& rm : rmMap)
                {
                    t = rm.first;
                    ClosestInfo info{};
                    float3 NxDelta = NxD + t * NxM;
                    if (NxDelta != vzero)
                    {
                        GetPair(line, circle, D, t, info.lineClosest, info.circleClosest);
                        info.equidistant = false;
                    }
                    else
                    {
                        float3 U = GetOrthogonal(circleNormal, true);
                        info.lineClosest = circleCenter;
                        info.circleClosest = circleCenter + circleRadius * U;
                        info.equidistant = true;
                    }
                    float3 diff = info.lineClosest - info.circleClosest;
                    info.sqrDistance = dot(diff, diff);
                    candidates[numRoots++] = info;
                }

                std::sort(candidates.begin(), candidates.begin() + numRoots);

                result.numClosestPairs = 1;
                result.lineClosest[0] = candidates[0].lineClosest;
                result.circleClosest[0] = candidates[0].circleClosest;
                if (numRoots > 1 &&
                    candidates[1].sqrDistance == candidates[0].sqrDistance)
                {
                    result.numClosestPairs = 2;
                    result.lineClosest[1] = candidates[1].lineClosest;
                    result.circleClosest[1] = candidates[1].circleClosest;
                }
            }
            else
            {
                // The line is parallel to the plane of the circle.
                // The polynomial has the form
                // H(t) = (t+v)^2*[(t+v)^2-(r^2-u^2)].
                float u = dot(NxM, D), v = dot(lineDirection, D);
                float discr = circleRadius * circleRadius - u * u;
                if (discr > 0.0)
                {
                    result.numClosestPairs = 2;
                    float rootDiscr = std::sqrt(discr);
                    t = -v + rootDiscr;
                    GetPair(line, circle, D, t, result.lineClosest[0],
                        result.circleClosest[0]);
                    t = -v - rootDiscr;
                    GetPair(line, circle, D, t, result.lineClosest[1],
                        result.circleClosest[1]);
                }
                else
                {
                    result.numClosestPairs = 1;
                    t = -v;
                    GetPair(line, circle, D, t, result.lineClosest[0],
                        result.circleClosest[0]);
                }
            }
        }
        else
        {
            // The line is C+t*M, where M is not parallel to N.  The
            // polynomial is
            // H(t) = |cross(N,M)|^2*t^2*(t^2 - r^2*|cross(N,M)|^2)
            // where root t = 0 does not correspond to the global
            // minimum.  The other roots produce the global minimum.
            result.numClosestPairs = 2;
            t = circleRadius * Length(NxM);
            GetPair(line, circle, D, t, result.lineClosest[0],
                result.circleClosest[0]);
            t = -t;
            GetPair(line, circle, D, t, result.lineClosest[1],
                result.circleClosest[1]);
        }
        result.equidistant = false;
    }
    else
    {
        if (NxD != vzero)
        {
            // The line is A+t*N (perpendicular to plane) but with
            // A != C.  The polyhomial is
            // H(t) = |cross(N,D)|^2*(t + dot(M,D))^2.
            result.numClosestPairs = 1;
            t = -dot(lineDirection, D);
            GetPair(line, circle, D, t, result.lineClosest[0],
                result.circleClosest[0]);
            result.equidistant = false;
        }
        else
        {
            // The line is C+t*N, so C is the closest point for the
            // line and all circle points are equidistant from it.
            float3 U = GetOrthogonal(circleNormal, true);
            result.numClosestPairs = 1;
            result.lineClosest[0] = circleCenter;
            result.circleClosest[0] = circleCenter + circleRadius * U;
            result.equidistant = true;
        }
    }

    float3 diff = result.lineClosest[0] - result.circleClosest[0];
    result.sqrDistance = dot(diff, diff);
    result.distance = std::sqrt(result.sqrDistance);
    return result;
}