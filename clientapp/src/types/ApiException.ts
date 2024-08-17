import { HttpStatusCode } from "axios";

export interface ApiException
{
    responseStatusCode?: HttpStatusCode,
    responseHeaders?: any
}