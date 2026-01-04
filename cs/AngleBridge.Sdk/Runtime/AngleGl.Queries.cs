using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct QueriesApi
        {
            private readonly AngleGl _gl;

            internal QueriesApi(AngleGl gl)
            {
                _gl = gl;
            }

            public bool IsDisjointTimerQueryAvailable
            {
                get
                {
                    _gl.Guard();
                    return Gles.IsDisjointTimerQueryAvailable || Gles.TryGetInteger(Gles.GL_QUERY_RESULT_AVAILABLE, out _);
                }
            }

            public AngleQuery CreateQueryObject()
            {
                _gl.Guard();
                var handle = new GlQueryHandle(I32(Gles.GenQuery()));
                return new AngleQuery(_gl, handle);
            }

            public void DeleteQuery(GlQueryHandle query)
            {
                _gl.Guard();
                if (query.IsNull) return;
                Gles.DeleteQuery(U32(query.Value));
            }

            public void DeleteQuery(AngleQuery query)
            {
                _gl.Guard();
                _gl.ValidateQuery(query);
                Gles.DeleteQuery(U32(query.Handle.Value));
                query.Dispose();
            }

            public void BeginQuery(QueryTarget target, AngleQuery query)
            {
                _gl.Guard();
                _gl.ValidateQuery(query);
                Gles.BeginQuery(U32((int)target), U32(query.Handle.Value));
            }

            public void EndQuery(QueryTarget target)
            {
                _gl.Guard();
                Gles.EndQuery(U32((int)target));
            }

            public bool TryGetQueryResultAvailable(AngleQuery query, out bool available)
            {
                _gl.Guard();
                _gl.ValidateQuery(query);
                if (Gles.TryGetQueryObjectuiv(U32(query.Handle.Value), Gles.GL_QUERY_RESULT_AVAILABLE, out var val))
                {
                    available = val != 0;
                    return true;
                }
                available = false;
                return false;
            }

            [CLSCompliant(false)]
            public bool TryGetQueryResult(AngleQuery query, out ulong result)
            {
                _gl.Guard();
                _gl.ValidateQuery(query);
                if (Gles.TryGetQueryObjectui64v(U32(query.Handle.Value), Gles.GL_QUERY_RESULT, out var val))
                {
                    result = val;
                    return true;
                }
                result = 0;
                return false;
            }

            public int CreateTimerQuery()
            {
                _gl.Guard();
                return I32(Gles.GenQuery());
            }

            public void DeleteTimerQuery(int query)
            {
                _gl.Guard();
                if (query == 0) return;
                Gles.DeleteQuery(U32(query));
            }

            public void BeginTimerQuery(int query)
            {
                _gl.Guard();
                if (query <= 0) throw new ArgumentOutOfRangeException(nameof(query), "Query id must be positive.");
                Gles.BeginQuery(Gles.GL_TIME_ELAPSED, U32(query));
            }

            public void EndTimerQuery()
            {
                _gl.Guard();
                Gles.EndQuery(Gles.GL_TIME_ELAPSED);
            }

            public bool TryGetTimerQueryResultAvailable(int query, out bool available)
            {
                _gl.Guard();
                if (query <= 0) throw new ArgumentOutOfRangeException(nameof(query), "Query id must be positive.");
                if (Gles.TryGetQueryObjectuiv(U32(query), Gles.GL_QUERY_RESULT_AVAILABLE, out var val))
                {
                    available = val != 0;
                    return true;
                }
                available = false;
                return false;
            }

            public bool TryGetTimerQueryResult(int query, out long nanoseconds)
            {
                _gl.Guard();
                if (query <= 0) throw new ArgumentOutOfRangeException(nameof(query), "Query id must be positive.");
                if (Gles.TryGetQueryObjectui64v(U32(query), Gles.GL_QUERY_RESULT, out var ns))
                {
                    nanoseconds = ns > long.MaxValue ? long.MaxValue : (long)ns;
                    return true;
                }

                nanoseconds = 0;
                return false;
            }
        }
    }
}
