import { APP_URL_Home } from "../constants/general";
import { Box, Button, CssBaseline, Typography } from "@mui/material"
import { useInterval } from "usehooks-ts";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { yellow } from "@mui/material/colors";
import DirectionsOffIcon from '@mui/icons-material/DirectionsOff';

export const RouteNotFound = () => {
    const [secToRedirect, setSecToRedirect] = useState(3)
    const [cancelAutoRedirect, setCancelAutoRedirect] = useState(false)
    const navigate = useNavigate()

    useInterval(() => {
        if (cancelAutoRedirect) return

        if (secToRedirect > 0)
            setSecToRedirect(secToRedirect - 1)
        else
            navigate(APP_URL_Home)
    }, 1000)

    return <Box sx={{ m: 1 }}>
        <CssBaseline />

        <Box sx={{ width: '100%' }}>

            <Box sx={{ p: 1, justifyContent: 'center', background: '#404040', textAlign: 'center' }}>
                <DirectionsOffIcon fontSize="large" />

                <Typography color={yellow[400]}>
                    {String(document.location)}
                </Typography>
                <Typography>
                    Route not found
                </Typography>

                {cancelAutoRedirect === false && <Box>
                    <br />
                    <Typography sx={{ verticalAlign: 'center' }}>
                        Autoredirect in {secToRedirect} seconds...
                    </Typography>
                    <Button onClick={() => { setCancelAutoRedirect(true) }}>
                        Cancel
                    </Button>
                </Box>}

                {cancelAutoRedirect === true && <Box>
                    <Button onClick={() => { navigate(APP_URL_Home) }}>
                        Back to home
                    </Button>
                </Box>}
            </Box>
        </Box>
    </Box>
}
