#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class NetSprotoTcpSocketWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Net.SprotoTcpSocket);
			Utils.BeginObjectRegister(type, L, translator, 0, 4, 4, 4);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SendRequest", _m_SendRequest);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SendResponse", _m_SendResponse);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnConnect", _e_OnConnect);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnClose", _e_OnClose);
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "Log", _g_get_Log);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "TcpSocket", _g_get_TcpSocket);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Dispatcher", _g_get_Dispatcher);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Proto", _g_get_Proto);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "Log", _s_set_Log);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "TcpSocket", _s_set_TcpSocket);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Dispatcher", _s_set_Dispatcher);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Proto", _s_set_Proto);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 4 && (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING) && (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING) && LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 4))
				{
					string _fileS2C = LuaAPI.lua_tostring(L, 2);
					string _fileC2S = LuaAPI.lua_tostring(L, 3);
					bool _isbinary = LuaAPI.lua_toboolean(L, 4);
					
					Net.SprotoTcpSocket gen_ret = new Net.SprotoTcpSocket(_fileS2C, _fileC2S, _isbinary);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 3 && (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING) && (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING))
				{
					string _fileS2C = LuaAPI.lua_tostring(L, 2);
					string _fileC2S = LuaAPI.lua_tostring(L, 3);
					
					Net.SprotoTcpSocket gen_ret = new Net.SprotoTcpSocket(_fileS2C, _fileC2S);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 3 && translator.Assignable<Sproto.SprotoMgr>(L, 2) && translator.Assignable<Sproto.SprotoMgr>(L, 3))
				{
					Sproto.SprotoMgr _s2C = (Sproto.SprotoMgr)translator.GetObject(L, 2, typeof(Sproto.SprotoMgr));
					Sproto.SprotoMgr _c2S = (Sproto.SprotoMgr)translator.GetObject(L, 3, typeof(Sproto.SprotoMgr));
					
					Net.SprotoTcpSocket gen_ret = new Net.SprotoTcpSocket(_s2C, _c2S);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to Net.SprotoTcpSocket constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendRequest(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Net.SprotoTcpSocket gen_to_be_invoked = (Net.SprotoTcpSocket)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<Sproto.SprotoObject>(L, 3)&& translator.Assignable<Net.ProtoDispatcher.MessageHandler>(L, 4)) 
                {
                    string _proto = LuaAPI.lua_tostring(L, 2);
                    Sproto.SprotoObject _request = (Sproto.SprotoObject)translator.GetObject(L, 3, typeof(Sproto.SprotoObject));
                    Net.ProtoDispatcher.MessageHandler _handler = translator.GetDelegate<Net.ProtoDispatcher.MessageHandler>(L, 4);
                    
                    gen_to_be_invoked.SendRequest( _proto, _request, _handler );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<Sproto.SprotoObject>(L, 3)) 
                {
                    string _proto = LuaAPI.lua_tostring(L, 2);
                    Sproto.SprotoObject _request = (Sproto.SprotoObject)translator.GetObject(L, 3, typeof(Sproto.SprotoObject));
                    
                    gen_to_be_invoked.SendRequest( _proto, _request );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _proto = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.SendRequest( _proto );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Net.SprotoTcpSocket.SendRequest!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendResponse(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Net.SprotoTcpSocket gen_to_be_invoked = (Net.SprotoTcpSocket)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _proto = LuaAPI.lua_tostring(L, 2);
                    Sproto.SprotoObject _response = (Sproto.SprotoObject)translator.GetObject(L, 3, typeof(Sproto.SprotoObject));
                    long _session = LuaAPI.lua_toint64(L, 4);
                    
                    gen_to_be_invoked.SendResponse( _proto, _response, _session );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Log(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Net.SprotoTcpSocket gen_to_be_invoked = (Net.SprotoTcpSocket)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Log);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_TcpSocket(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Net.SprotoTcpSocket gen_to_be_invoked = (Net.SprotoTcpSocket)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.TcpSocket);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Dispatcher(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Net.SprotoTcpSocket gen_to_be_invoked = (Net.SprotoTcpSocket)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Dispatcher);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Proto(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Net.SprotoTcpSocket gen_to_be_invoked = (Net.SprotoTcpSocket)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Proto);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Log(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Net.SprotoTcpSocket gen_to_be_invoked = (Net.SprotoTcpSocket)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Log = translator.GetDelegate<Net.SprotoTcpSocket.LogType>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_TcpSocket(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Net.SprotoTcpSocket gen_to_be_invoked = (Net.SprotoTcpSocket)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.TcpSocket = (Net.TcpClientSocket)translator.GetObject(L, 2, typeof(Net.TcpClientSocket));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Dispatcher(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Net.SprotoTcpSocket gen_to_be_invoked = (Net.SprotoTcpSocket)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Dispatcher = (Net.ProtoDispatcher)translator.GetObject(L, 2, typeof(Net.ProtoDispatcher));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Proto(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Net.SprotoTcpSocket gen_to_be_invoked = (Net.SprotoTcpSocket)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Proto = (Sproto.SprotoRpc)translator.GetObject(L, 2, typeof(Sproto.SprotoRpc));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_OnConnect(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			Net.SprotoTcpSocket gen_to_be_invoked = (Net.SprotoTcpSocket)translator.FastGetCSObj(L, 1);
                System.Action<Net.SprotoTcpSocket> gen_delegate = translator.GetDelegate<System.Action<Net.SprotoTcpSocket>>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need System.Action<Net.SprotoTcpSocket>!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.OnConnect += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.OnConnect -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to Net.SprotoTcpSocket.OnConnect!");
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_OnClose(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int gen_param_count = LuaAPI.lua_gettop(L);
			Net.SprotoTcpSocket gen_to_be_invoked = (Net.SprotoTcpSocket)translator.FastGetCSObj(L, 1);
                System.Action<Net.SprotoTcpSocket> gen_delegate = translator.GetDelegate<System.Action<Net.SprotoTcpSocket>>(L, 3);
                if (gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need System.Action<Net.SprotoTcpSocket>!");
                }
				
				if (gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						gen_to_be_invoked.OnClose += gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						gen_to_be_invoked.OnClose -= gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to Net.SprotoTcpSocket.OnClose!");
            return 0;
        }
        
		
		
    }
}
