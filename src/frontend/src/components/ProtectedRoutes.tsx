// https://github.com/remix-run/react-router/issues/10637#issuecomment-1802180978

import { APP_URL_Login } from '../constants/general';
import { Navigate } from 'react-router'
import { Outlet } from "react-router-dom";
import Layout from './Layout';
import { useGlobalPersistService } from '../services/globalPersistService';
import { useGlobalService } from '../services/globalService';

const ProtectedRoutes = () => {
    const globalState = useGlobalService()
    const globalPersistState = useGlobalPersistService()

    const loginRedirectUrlFrom = () => {
        if (location.pathname !== APP_URL_Login())
            return encodeURIComponent(location.pathname)
    }

    return (globalPersistState.currentUser != null)
        ?
        <Layout child={<Outlet />} />
        :
        <Navigate to={APP_URL_Login(loginRedirectUrlFrom())} replace />;
};

export default ProtectedRoutes;